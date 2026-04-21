import React, { useEffect, useState } from 'react';
import { Table, Button, Input, Upload, message, Space, Card, Modal, Form, InputNumber, Select } from 'antd';
import { UploadOutlined, SearchOutlined, PlusOutlined } from '@ant-design/icons';
import axios from 'axios';

const { Option } = Select;

const ListaProductos = () => {
  const [data, setData] = useState([]);
  const [categorias, setCategorias] = useState([]);
  const [loading, setLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingId, setEditingId] = useState(null); 
  const [form] = Form.useForm();
  const [pagination, setPagination] = useState({ current: 1, pageSize: 10, total: 0 });
  const [searchText, setSearchText] = useState('');

  // Requisito: Paginación y filtros desde el backend [cite: 7, 27, 83]
  const fetchProductos = async (page = 1, pageSize = 10, search = '') => {
    setLoading(true);
    try {
      const response = await axios.get(`http://localhost:5220/api/productos`, {
        params: { page, pageSize, search }
      });
      setData(response.data.items);
      setPagination({ current: page, pageSize, total: response.data.total });
    } catch (error) {
      message.error("Error al cargar productos");
    } finally {
      setLoading(false);
    }
  };

  const fetchCategorias = async () => {
    try {
      const response = await axios.get('http://localhost:5220/api/categorias');
      setCategorias(response.data);
    } catch (error) {
      console.error("Error cargando categorías");
    }
  };

  useEffect(() => {
    fetchProductos();
    fetchCategorias();
  }, []);

  // --- FUNCIÓN ELIMINAR (Requisito CRUD completo) [cite: 29, 80] ---
  const handleEliminar = (id) => {
    Modal.confirm({
      title: '¿Estás seguro de eliminar este producto?',
      okText: 'Sí, eliminar',
      okType: 'danger',
      onOk: async () => {
        try {
          await axios.delete(`http://localhost:5220/api/productos/${id}`);
          message.success("Producto eliminado");
          fetchProductos(pagination.current, pagination.pageSize, searchText);
        } catch (error) {
          message.error("Error al eliminar");
        }
      },
    });
  };

  // --- FUNCIÓN ACTUALIZAR (Requisito Editar) [cite: 29, 79] ---
  const handleEditClick = (record) => {
    setEditingId(record.idProducto);
    form.setFieldsValue(record); 
    setIsModalOpen(true);
  };

  const handleSubmit = async (values) => {
    try {
      if (editingId) {
        // PUT para editar [cite: 116]
        await axios.put(`http://localhost:5220/api/productos/${editingId}`, values);
        message.success("Producto actualizado");
      } else {
        // POST para crear [cite: 114]
        await axios.post('http://localhost:5220/api/productos', values);
        message.success("Producto creado");
      }
      setIsModalOpen(false);
      setEditingId(null);
      form.resetFields();
      fetchProductos(pagination.current, pagination.pageSize, searchText);
    } catch (error) {
      message.error("Error en la operación");
    }
  };

  const columns = [
    { title: 'Nombre', dataIndex: 'nombre', key: 'nombre' },
    { title: 'Precio', dataIndex: 'precio', key: 'precio', render: (val) => `$${val.toLocaleString()}` },
    { title: 'Stock', dataIndex: 'stock', key: 'stock' },
    {
      title: 'Acciones',
      key: 'acciones',
      render: (_, record) => (
        <Space>
          <Button type="link" onClick={() => handleEditClick(record)}>Editar</Button>
          <Button type="link" danger onClick={() => handleEliminar(record.idProducto)}>Eliminar</Button>
        </Space>
      ),
    },
  ];

  return (
    <Card title="Administración de Productos" style={{ margin: '20px' }}>
      <Space direction="vertical" style={{ width: '100%' }} size="large">
        <Space style={{ justifyContent: 'space-between', width: '100%' }}>
          <Space>
            <Input 
              placeholder="Buscar por nombre..." 
              prefix={<SearchOutlined />} 
              onChange={(e) => {
                setSearchText(e.target.value);
                fetchProductos(1, pagination.pageSize, e.target.value);
              }}
            />
            {/* Requisito: Carga masiva [cite: 8, 115] */}
            <Upload name="file" action="http://localhost:5220/api/productos/upload" showUploadList={false} onChange={(info) => {
              if (info.file.status === 'done') { fetchProductos(); message.success("Excel cargado"); }
            }}>
              <Button icon={<UploadOutlined />}>Cargar Excel</Button>
            </Upload>
          </Space>
          <Button type="primary" icon={<PlusOutlined />} onClick={() => { setEditingId(null); form.resetFields(); setIsModalOpen(true); }}>
            Nuevo Producto
          </Button>
        </Space>

        <Table 
          columns={columns} 
          dataSource={data} 
          rowKey="idProducto" 
          pagination={pagination}
          loading={loading}
          onChange={(pag) => fetchProductos(pag.current, pag.pageSize, searchText)}
        />
      </Space>

      <Modal 
        title={editingId ? "Editar Producto" : "Crear Nuevo Producto"} 
        open={isModalOpen} 
        onCancel={() => { setIsModalOpen(false); setEditingId(null); }} 
        onOk={() => form.submit()}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit}>
          <Form.Item name="nombre" label="Nombre" rules={[{ required: true, message: 'Requerido' }]}>
            <Input />
          </Form.Item>
          <Form.Item name="idCategoria" label="Categoría" rules={[{ required: true }]}>
            <Select placeholder="Seleccione">
              {categorias.map(cat => <Option key={cat.idCategoria} value={cat.idCategoria}>{cat.nombre}</Option>)}
            </Select>
          </Form.Item>
          <Form.Item name="precio" label="Precio" rules={[{ required: true }, { type: 'number', min: 0.01, message: 'Precio > 0' }]}>
            <InputNumber style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="stock" label="Stock" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
};

export default ListaProductos;